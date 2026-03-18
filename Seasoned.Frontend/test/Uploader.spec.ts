import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import { createRouter, createWebHistory } from 'vue-router'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import Uploader from "@/pages/uploader.vue"

const vuetify = createVuetify({ components, directives })

global.ResizeObserver = class ResizeObserver {
  observe() {} unobserve() {} disconnect() {}
};

vi.stubGlobal('useRuntimeConfig', () => ({
  public: { apiBase: 'http://localhost:5000/' }
}))

const mockRouter = { push: vi.fn() }
vi.stubGlobal('useRouter', () => mockRouter)

const mockFetch = vi.fn()
vi.stubGlobal('$fetch', mockFetch)

const router = createRouter({
  history: createWebHistory(),
  routes: [],
})

describe('Uploader.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  const mountOptions = {
    global: { 
      plugins: [vuetify, router],
      stubs: { RecipeDisplay: true }, 
      provide: { 'router': mockRouter } 
    }
  }
  
  it('renders the drop zone and upload button', () => {
    const wrapper = mount(Uploader, mountOptions)
    expect(wrapper.text()).toContain('Analyze Recipe')
  })

  it('shows the filename when a file is selected', async () => {
    const wrapper = mount(Uploader, mountOptions)
    const file = new File(['(data)'], 'grandmas-cookies.png', { type: 'image/png' })
    const vm = wrapper.vm as any
    
    vm.files = [file]
    
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('grandmas-cookies.png')
  })

  it('shows loading state on the button when analyzing', async () => {
    const wrapper = mount(Uploader, mountOptions)
    const vm = wrapper.vm as any
    
    vm.loading = true
    await wrapper.vm.$nextTick()

    const btn = wrapper.find('.analyze-btn')
    expect(btn.attributes('class')).toContain('v-btn--loading')
  })

  it('restores a recipe from localStorage on mount', async () => {
    const savedRecipe = { title: 'Restored Cake', ingredients: [], instructions: [] }
    localStorage.setItem('pending_recipe', JSON.stringify(savedRecipe))

    const wrapper = mount(Uploader, mountOptions)
    const vm = wrapper.vm as any
    
    expect(vm.recipe.title).toBe('Restored Cake')
    expect(localStorage.getItem('pending_recipe')).toBeNull()
  })
  
})
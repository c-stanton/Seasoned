import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { nextTick } from 'vue'
import { createVuetify } from 'vuetify'
import { flushPromises } from '@vue/test-utils'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import GalleryPage from "@/pages/gallery.vue" 

const vuetify = createVuetify({ components, directives })

global.ResizeObserver = class ResizeObserver {
  observe() {} unobserve() {} disconnect() {}
};

global.visualViewport = {
  width: 1024,
  height: 768,
  offsetLeft: 0,
  offsetTop: 0,
  pageLeft: 0,
  pageTop: 0,
  scale: 1,
  addEventListener: vi.fn(),
  removeEventListener: vi.fn(),
  dispatchEvent: vi.fn(),
} as unknown as VisualViewport;

vi.stubGlobal('useRuntimeConfig', () => ({
  public: { apiBase: 'http://localhost:5000/' }
}))

const mockFetch = vi.fn()
vi.stubGlobal('$fetch', mockFetch)

const mockNavigate = vi.fn()
vi.stubGlobal('navigateTo', mockNavigate)

describe('GalleryPage.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockFetch.mockResolvedValue([])
  })

  const mountOptions = {
    global: { plugins: [vuetify] }
  }

  it('shows loading state initially and then renders recipes', async () => {
    mockFetch.mockResolvedValueOnce([
      { id: 1, title: 'Bolognese', createdAt: new Date().toISOString(), ingredients: [], instructions: [] }
    ])

    const wrapper = mount(GalleryPage, mountOptions)
    
    expect(wrapper.text()).toContain('Opening Collection...')

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Bolognese')
    })
  })

  it('triggers semantic search when searchQuery changes', async () => {
    vi.useFakeTimers()

    mockFetch.mockResolvedValueOnce([]) 

    const wrapper = mount(GalleryPage, mountOptions)
    
    const vm = wrapper.vm as any
    vm.searchQuery = 'spicy pasta'

    await nextTick()
    
    vi.advanceTimersByTime(600)
    
    mockFetch.mockResolvedValueOnce([{ id: 2, title: 'Spicy Pasta' }])

    await flushPromises()

    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('api/recipe/search'),
      expect.objectContaining({ 
        query: { query: 'spicy pasta' },
        credentials: 'include'
      })
    )

    vi.useRealTimers()
  })

  it('redirects to login if API returns 401', async () => {
    mockFetch.mockReset()
    mockFetch.mockRejectedValue({ status: 401 })

    mount(GalleryPage, mountOptions)

    await flushPromises()

    await vi.waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/login')
    }, { timeout: 1000 })
  })

  it('enters editing mode and formats arrays into strings', async () => {
    mockFetch.mockResolvedValueOnce([
      { 
        id: 1, 
        title: 'Muffins', 
        ingredients: ['Flour', 'Sugar'], 
        instructions: ['Mix', 'Bake'],
        createdAt: new Date().toISOString() 
      }
    ])

    const wrapper = mount(GalleryPage, mountOptions)
    await vi.waitFor(() => expect(wrapper.vm.recipes.length).toBe(1))

    const vm = wrapper.vm as any
    vm.editRecipe(vm.recipes[0])

    expect(vm.isEditing).toBe(true)
    expect(vm.selectedRecipe.ingredients).toBe('Flour\nSugar')
  })

  it('shoves updated recipe back to .NET API on saveChanges', async () => {
    const mockRecipe = { id: 1, title: 'Old Title', ingredients: 'Water', instructions: 'Boil', createdAt: new Date().toISOString() }
    mockFetch.mockResolvedValueOnce([mockRecipe])

    const wrapper = mount(GalleryPage, mountOptions)
    await vi.waitFor(() => expect(wrapper.vm.recipes.length).toBe(1))

    const vm = wrapper.vm as any
    vm.selectedRecipe = { ...mockRecipe, title: 'New Title' }
    vm.isEditing = true

    mockFetch.mockResolvedValueOnce({ success: true })

    await vm.saveChanges()

    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('api/recipe/update/1'),
      expect.objectContaining({ 
        method: 'PUT',
        credentials: 'include'
      })
    )
    expect(vm.recipes[0].title).toBe('New Title')
  })
})